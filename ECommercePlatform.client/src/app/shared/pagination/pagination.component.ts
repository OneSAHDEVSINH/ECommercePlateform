import { Component, EventEmitter, Input, OnChanges, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.scss']
})
export class PaginationComponent implements OnChanges {
  @Input() totalPages: number = 0;
  @Input() currentPage: number = 1;
  @Input() totalCount: number = 0;
  @Input() pageSize: number = 10;
  @Input() pageNumber: number = 1; 
  @Output() pageChange = new EventEmitter<number>();

  pages: number[] = [];

  ngOnChanges(): void {
    // If totalCount and pageSize are provided, calculate totalPages
    if (this.totalCount > 0 && this.pageSize > 0) {
      this.totalPages = Math.ceil(this.totalCount / this.pageSize);
    }

    this.generatePagination();
  }

  generatePagination(): void {
    this.pages = [];
    const maxPagesToShow = 5;

    if (this.totalPages <= maxPagesToShow) {
      // Show all pages
      for (let i = 1; i <= this.totalPages; i++) {
        this.pages.push(i);
      }
    } else {
      // Show subset of pages with ellipsis
      if (this.currentPage <= 3) {
        // Near the start
        for (let i = 1; i <= 4; i++) {
          this.pages.push(i);
        }
        this.pages.push(-1); // Represents ellipsis
        this.pages.push(this.totalPages);
      } else if (this.currentPage >= this.totalPages - 2) {
        // Near the end
        this.pages.push(1);
        this.pages.push(-1);
        for (let i = this.totalPages - 3; i <= this.totalPages; i++) {
          this.pages.push(i);
        }
      } else {
        // Somewhere in the middle
        this.pages.push(1);
        this.pages.push(-1);
        for (let i = this.currentPage - 1; i <= this.currentPage + 1; i++) {
          this.pages.push(i);
        }
        this.pages.push(-1);
        this.pages.push(this.totalPages);
      }
    }
  }

  changePage(page: number): void {
    if (page !== this.currentPage && page > 0 && page <= this.totalPages) {
      this.currentPage = page;
      this.pageChange.emit(page);
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.changePage(this.currentPage - 1);
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.changePage(this.currentPage + 1);
    }
  }
}
