using FigureProj.Common.Models;
using FigureProj.Common.Models.Abstract;
using FigureProj.Common.Services;

namespace FigureProj.Tests
{
    public class CrudServiceAsyncTests
    {
        // Тест створення елемента
        [Fact]
        public async Task CreateAsync_ShouldAddElement_WhenValidElement()
        {
            // Arrange
            var service = new CrudServiceAsync<Figure>("test_create.json");
            var circle = Circle.CreateNew();

            // Act
            var result = await service.CreateAsync(circle);

            // Assert
            Assert.True(result);
            Assert.Equal(1, service.Count);
        }

        // Тест створення дубліката
        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenDuplicateId()
        {
            // Arrange
            var service = new CrudServiceAsync<Figure>("test_duplicate.json");
            var circle = new Circle(5.0, "Коло", "синій");
            await service.CreateAsync(circle);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await service.CreateAsync(circle);
            });
        }

        // Тест читання елемента
        [Fact]
        public async Task ReadAsync_ShouldReturnElement_WhenElementExists()
        {
            // Arrange
            var service = new CrudServiceAsync<Figure>("test_read.json");
            var square = Square.CreateNew();
            await service.CreateAsync(square);

            // Act
            var result = await service.ReadAsync(square.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(square.Id, result.Id);
        }

        // Тест читання неіснуючого елемента
        [Fact]
        public async Task ReadAsync_ShouldThrowException_WhenElementNotFound()
        {
            // Arrange
            var service = new CrudServiceAsync<Figure>("test_read_notfound.json");
            var randomId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await service.ReadAsync(randomId);
            });
        }

        // Тест читання всіх елементів
        [Fact]
        public async Task ReadAllAsync_ShouldReturnAllElements()
        {
            // Arrange
            var service = new CrudServiceAsync<Figure>("test_readall.json");
            await service.CreateAsync(Circle.CreateNew());
            await service.CreateAsync(Square.CreateNew());
            await service.CreateAsync(Rectangle.CreateNew());

            // Act
            var result = await service.ReadAllAsync();

            // Assert
            Assert.Equal(3, result.Count());
        }

        // Тест пагінації
        [Fact]
        public async Task ReadAllAsync_WithPagination_ShouldReturnCorrectPage()
        {
            // Arrange
            var service = new CrudServiceAsync<Figure>("test_pagination.json");
            for (int i = 0; i < 25; i++)
            {
                await service.CreateAsync(Circle.CreateNew());
            }

            // Act
            var page1 = await service.ReadAllAsync(1, 10);
            var page2 = await service.ReadAllAsync(2, 10);
            var page3 = await service.ReadAllAsync(3, 10);

            // Assert
            Assert.Equal(10, page1.Count());
            Assert.Equal(10, page2.Count());
            Assert.Equal(5, page3.Count());
        }

        // Тест пагінації з невалідними параметрами
        [Fact]
        public async Task ReadAllAsync_WithInvalidPage_ShouldThrowException()
        {
            // Arrange
            var service = new CrudServiceAsync<Figure>("test_invalid_page.json");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                await service.ReadAllAsync(0, 10);
            });
        }

        // Тест оновлення елемента
        [Fact]
        public async Task UpdateAsync_ShouldUpdateElement_WhenElementExists()
        {
            // Arrange
            var service = new CrudServiceAsync<Figure>("test_update.json");
            var circle = new Circle(5.0, "Коло", "синій");
            await service.CreateAsync(circle);

            // Act
            circle.Radius = 10.0;
            var result = await service.UpdateAsync(circle);

            // Assert
            Assert.True(result);
            var updated = await service.ReadAsync(circle.Id);
            Assert.Equal(10.0, ((Circle)updated).Radius);
        }

        // Тест оновлення неіснуючого елемента
        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenElementNotFound()
        {
            // Arrange
            var service = new CrudServiceAsync<Figure>("test_update_notfound.json");
            var circle = new Circle(5.0, "Коло", "синій");

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await service.UpdateAsync(circle);
            });
        }

        // Тест видалення елемента
        [Fact]
        public async Task RemoveAsync_ShouldRemoveElement_WhenElementExists()
        {
            // Arrange
            var service = new CrudServiceAsync<Figure>("test_remove.json");
            var triangle = Triangle.CreateNew();
            await service.CreateAsync(triangle);

            // Act
            var result = await service.RemoveAsync(triangle);

            // Assert
            Assert.True(result);
            Assert.Equal(0, service.Count);
        }

        // Тест видалення неіснуючого елемента
        [Fact]
        public async Task RemoveAsync_ShouldThrowException_WhenElementNotFound()
        {
            // Arrange
            var service = new CrudServiceAsync<Figure>("test_remove_notfound.json");
            var circle = new Circle(5.0, "Коло", "синій");

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await service.RemoveAsync(circle);
            });
        }

        // Тест збереження у файл
        [Fact]
        public async Task SaveAsync_ShouldSaveToFile()
        {
            // Arrange
            var filePath = "test_save.json";
            var service = new CrudServiceAsync<Figure>(filePath);
            await service.CreateAsync(Circle.CreateNew());
            await service.CreateAsync(Square.CreateNew());

            // Act
            var result = await service.SaveAsync();

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(filePath));

            // Cleanup
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        // Тест багатопотокової безпеки
        [Fact]
        public async Task CreateAsync_ShouldBeThreadSafe_WhenConcurrentCalls()
        {
            // Arrange
            var service = new CrudServiceAsync<Figure>("test_threadsafe.json");
            var tasks = new List<Task>();

            // Act
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var figure = Circle.CreateNew();
                    await service.CreateAsync(figure);
                }));
            }

            await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(100, service.Count);
        }

        // Тест IEnumerable
        [Fact]
        public async Task GetEnumerator_ShouldIterateAllElements()
        {
            // Arrange
            var service = new CrudServiceAsync<Figure>("test_enumerable.json");
            await service.CreateAsync(Circle.CreateNew());
            await service.CreateAsync(Square.CreateNew());
            await service.CreateAsync(Rectangle.CreateNew());

            // Act
            int count = 0;
            foreach (var figure in service)
            {
                count++;
            }

            // Assert
            Assert.Equal(3, count);
        }

        // Тест CreateNew для Circle
        [Fact]
        public void Circle_CreateNew_ShouldGenerateValidCircle()
        {
            // Act
            var circle = Circle.CreateNew();

            // Assert
            Assert.NotNull(circle);
            Assert.True(circle.Radius > 0);
            Assert.NotNull(circle.Name);
            Assert.NotNull(circle.Color);
        }

        // Тест CreateNew для Rectangle
        [Fact]
        public void Rectangle_CreateNew_ShouldGenerateValidRectangle()
        {
            // Act
            var rectangle = Rectangle.CreateNew();

            // Assert
            Assert.NotNull(rectangle);
            Assert.True(rectangle.Width > 0);
            Assert.True(rectangle.Height > 0);
            Assert.NotNull(rectangle.Name);
            Assert.NotNull(rectangle.Color);
        }

        // Тест CreateNew для Square
        [Fact]
        public void Square_CreateNew_ShouldGenerateValidSquare()
        {
            // Act
            var square = Square.CreateNew();

            // Assert
            Assert.NotNull(square);
            Assert.True(square.Side > 0);
            Assert.NotNull(square.Name);
            Assert.NotNull(square.Color);
        }

        // Тест CreateNew для Triangle
        [Fact]
        public void Triangle_CreateNew_ShouldGenerateValidTriangle()
        {
            // Act
            var triangle = Triangle.CreateNew();

            // Assert
            Assert.NotNull(triangle);
            Assert.True(triangle.A > 0);
            Assert.True(triangle.B > 0);
            Assert.True(triangle.C > 0);
            Assert.NotNull(triangle.Name);
            Assert.NotNull(triangle.Color);
        }

        // Тест паралельного створення багатьох елементів
        [Fact]
        public async Task CreateAsync_ShouldHandleMultipleConcurrentOperations()
        {
            // Arrange
            var service = new CrudServiceAsync<Figure>("test_concurrent.json");
            var tasks = new List<Task>();

            // Act
            for (int i = 0; i < 500; i++)
            {
                int index = i;
                tasks.Add(Task.Run(async () =>
                {
                    Figure figure = (index % 4) switch
                    {
                        0 => Circle.CreateNew(),
                        1 => Rectangle.CreateNew(),
                        2 => Square.CreateNew(),
                        _ => Triangle.CreateNew()
                    };
                    await service.CreateAsync(figure);
                }));
            }

            await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(500, service.Count);
        }
    }
}

